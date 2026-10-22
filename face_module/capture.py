
import face_recognition
import numpy as np
import base64
from io import BytesIO
from PIL import Image

def capture_face(face_image):

    try:

        # CHECK EMPTY

        if face_image is None:
            return None

        # REMOVE HEADER

        if "," in face_image:
            face_image = face_image.split(",")[1]

        # DECODE IMAGE

        image_data = base64.b64decode(face_image)

        image = Image.open(BytesIO(image_data))

        image = np.array(image)

        # FIND FACE

        encodings = face_recognition.face_encodings(image)

        if len(encodings) == 0:
            return None

        return encodings[0]

    except Exception as e:

        print("CAPTURE ERROR:", str(e))

        return None
