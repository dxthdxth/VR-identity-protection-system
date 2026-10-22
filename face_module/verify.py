
import face_recognition
import numpy as np
import base64
from io import BytesIO
from PIL import Image

def verify_face(stored_encoding, face_image):

    try:

        # CHECK EMPTY

        if stored_encoding is None or face_image is None:
            return False

        # REMOVE BASE64 HEADER

        if "," in face_image:
            face_image = face_image.split(",")[1]

        # DECODE IMAGE

        image_data = base64.b64decode(face_image)

        image = Image.open(BytesIO(image_data))

        image = np.array(image)

        # FIND FACE ENCODING

        encodings = face_recognition.face_encodings(image)

        if len(encodings) == 0:
            return False

        new_encoding = encodings[0]

        # CONVERT STORED DATA

        stored_encoding = np.array(
            stored_encoding,
            dtype=np.float64
        )

        # COMPARE FACE

        result = face_recognition.compare_faces(
            [stored_encoding],
            new_encoding,
            tolerance=0.5
        )

        return result[0]

    except Exception as e:

        print("VERIFY ERROR:", str(e))

        return False
