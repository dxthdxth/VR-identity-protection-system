from flask import Flask, request, jsonify, session, render_template, redirect
from flask_bcrypt import Bcrypt
from flask_cors import CORS
from models import db, User
import config
import uuid
from datetime import datetime, time, timedelta, timezone
import pickle
from threading import Lock

from face_module.capture import capture_face
from face_module.verify import verify_face

app = Flask(__name__)

app.config.from_object(config)
app.secret_key = getattr(config, "SECRET_KEY", "dev_secret_key")

CORS(app, supports_credentials=True)
db.init_app(app)
bcrypt = Bcrypt(app)

# ================= STORAGE =================
active_tokens = {}
meetings = {}
meeting_lock = Lock()
login_history = {}

# ================= TIME =================
IST = timezone(timedelta(hours=5, minutes=30))
ALLOWED_START = time(0, 0)
ALLOWED_END = time(23, 59)

def is_allowed_time():
    now_ist = datetime.now(IST).time()
    return ALLOWED_START <= now_ist <= ALLOWED_END

# ================= HOME =================
@app.route('/')
def home():
    return jsonify({"status": "running"})

# ================= REGISTER =================
@app.route('/register', methods=['GET', 'POST'])
def register():
    if request.method == 'POST':

        username = request.form.get('username')
        password_raw = request.form.get('password')
        face_image = request.form.get('face_image')

        if not username or not password_raw:
            return render_template('register.html', error="Missing fields")

        if not face_image:
            return render_template('register.html', error="Please capture face")

        if User.query.filter_by(username=username).first():
            return render_template('register.html', error="User already exists")

        password = bcrypt.generate_password_hash(password_raw).decode('utf-8')

        # ✅ FIXED HERE
        face_data = capture_face(face_image)

        if face_data is None:
            return render_template('register.html', error="No face detected")

        user = User(
            username=username,
            password=password,
            face_encoding=pickle.dumps(face_data),
            role="client"
        )

        db.session.add(user)
        db.session.commit()

        return redirect('/login')

    return render_template('register.html')

# ================= LOGIN =================
@app.route('/login', methods=['GET', 'POST'])
def login():
    if request.method == 'POST':

        username = request.form.get('username')
        password = request.form.get('password')
        face_image = request.form.get('face_image')

        if not username or not password:
            return render_template('login.html', error="Missing fields")

        user = User.query.filter_by(username=username).first()

        if not user:
            return render_template('login.html', error="Invalid user")

        if not bcrypt.check_password_hash(user.password, password):
            return render_template('login.html', error="Wrong password")

        if not face_image:
            return render_template('login.html', error="Please verify face")

        stored = pickle.loads(user.face_encoding)
        verified = verify_face(stored, face_image)

        if not verified:
            return render_template('login.html', error="Face not matched")

        # ================= SUCCESS LOGIN =================

        token = str(uuid.uuid4())
        active_tokens[username] = token

        session['user'] = username
        session['token'] = token

        # ================= LOGIN META DATA =================

        login_time = datetime.now().strftime("%d-%m-%Y %H:%M:%S")
        ip_address = request.headers.get('X-Forwarded-For', request.remote_addr)
        user_agent = request.headers.get('User-Agent', '')

        device = "Computer"
        browser = "Unknown"
        os_name = "Unknown"
        location = "India"

        # Browser detect
        if "Chrome" in user_agent:
            browser = "Chrome"
        elif "Firefox" in user_agent:
            browser = "Firefox"
        elif "Edge" in user_agent:
            browser = "Edge"
        elif "Safari" in user_agent:
            browser = "Safari"

        # OS detect
        if "Windows" in user_agent:
            os_name = "Windows"
        elif "Android" in user_agent:
            os_name = "Android"
        elif "Linux" in user_agent:
            os_name = "Linux"
        elif "Mac" in user_agent:
            os_name = "MacOS"

        # Device detect
        if "Mobile" in user_agent:
            device = "Mobile Phone"
        elif "Tablet" in user_agent:
            device = "Tablet"

        # ================= STORE IN SESSION (🔥 FIX) =================

        session['login_time'] = login_time
        session['ip_address'] = ip_address
        session['location'] = location
        session['device'] = device
        session['browser'] = browser
        session['os_name'] = os_name

        # ================= STORE HISTORY =================

        login_history.setdefault(username, []).append({
            "ip": ip_address,
            "location": location,
            "device": device,
            "browser": browser,
            "os": os_name,
            "time": login_time
        })

        return redirect('/dashboard')

    return render_template('login.html')

# ================= DASHBOARD =================
@app.route('/dashboard')
def dashboard():

    if 'user' not in session:
        return redirect('/login')

    username = session['user']
    token = session.get('token')

    user_logs = login_history.get(username, [])

    return render_template(
        'dashboard.html',
        user=username,
        token=token,
        login_count=len(user_logs),
        login_history=user_logs[::-1],

        # 🔥 FIXED SESSION VALUES
        login_time=session.get('login_time', 'N/A'),
        ip_address=session.get('ip_address', 'N/A'),
        location=session.get('location', 'India'),
        device=session.get('device', 'Unknown'),
        browser=session.get('browser', 'Unknown'),
        os_name=session.get('os_name', 'Unknown')
    )
# ================= CLEAR HISTORY =================
@app.route('/clear_history')
def clear_history():
    if 'user' not in session:
        return redirect('/login')

    login_history[session['user']] = []
    return redirect('/dashboard')

# ================= LOGOUT =================
@app.route('/logout')
def logout():
    username = session.get('user')
    active_tokens.pop(username, None)
    session.clear()
    return redirect('/login')

# ================= API LOGIN =================

@app.route('/api/login', methods=['POST'])
def api_login():

    try:

        if not is_allowed_time():

            return jsonify({

                "status": "fail",

                "message": "Login not allowed"

            }), 403

        data = request.get_json(silent=True) or {}

        username = data.get('username')

        password = data.get('password')

        if not username or not password:

            return jsonify({

                "status": "fail",

                "message": "Missing data"

            }), 400

        user = User.query.filter_by(
            username=username
        ).first()

        if not user:

            return jsonify({

                "status": "fail",

                "message": "User not found"

            }), 404

        if not bcrypt.check_password_hash(
            user.password,
            password
        ):

            return jsonify({

                "status": "fail",

                "message": "Wrong password"

            }), 401

        token = str(uuid.uuid4())

        active_tokens[username] = token

        return jsonify({

            "status": "success",

            "username": username,

            "token": token,

            "message": "Login successful"

        }), 200

    except Exception as e:

        print("LOGIN ERROR:", str(e))

        return jsonify({

            "status": "error",

            "message": str(e)

        }), 500


# ================= VERIFY TOKEN =================

@app.route('/api/verify_token', methods=['POST'])
def verify_token():

    data = request.get_json(silent=True) or {}

    username = data.get('username')

    token = data.get('token')

    if active_tokens.get(username) == token:

        return jsonify({
            "status": "success"
        }), 200

    return jsonify({
        "status": "fail"
    }), 401


# ================= CREATE MEETING =================

@app.route('/api/create_meeting', methods=['POST'])
def create_meeting():

    data = request.get_json(silent=True) or {}

    username = data.get("username")

    token = data.get("token")

    if not username or not token:

        return jsonify({

            "status": "fail",

            "message": "Missing data"

        }), 400

    if active_tokens.get(username) != token:

        return jsonify({

            "status": "fail",

            "message": "Invalid token"

        }), 401

    with meeting_lock:

        meeting_id = str(uuid.uuid4())[:6]

        meetings[meeting_id] = {

            "host": username,

            "created": datetime.now()

        }

    return jsonify({

        "status": "success",

        "meeting_id": meeting_id,

        "role": "Host",

        "message": "Meeting created"

    }), 200


# ================= JOIN MEETING =================

@app.route('/api/join_meeting', methods=['POST'])
def join_meeting():

    data = request.get_json(silent=True) or {}

    meeting_id = data.get(
        "meeting_id",
        ""
    ).strip()

    if not meeting_id:

        return jsonify({

            "status": "fail",

            "message": "Meeting ID empty"

        }), 400

    if meeting_id not in meetings:

        return jsonify({

            "status": "fail",

            "message": "Meeting not found"

        }), 404

    return jsonify({

        "status": "success",

        "role": "Client",

        "message": "Joined"

    }), 200


# ================= ERROR =================

@app.errorhandler(500)
def internal_error(e):

    return jsonify({

        "status": "error",

        "message": "Server error"

    }), 500

# ================= MAIN =================

if __name__ == "__main__":

    with app.app_context():

        db.create_all()

    app.run(

        host="0.0.0.0",

        port=5000,

        debug=True

    )