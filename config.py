import os

BASE_DIR = os.path.abspath(os.path.dirname(__file__))

# ✅ Database path
SQLALCHEMY_DATABASE_URI = "sqlite:///" + os.path.join(BASE_DIR, "app.db")

SQLALCHEMY_TRACK_MODIFICATIONS = False

SECRET_KEY = "identity_project_secret"