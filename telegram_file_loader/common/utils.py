import datetime
import uuid

import pytz
from telethon.tl.types import User


def uuid_str() -> str:
    return str(uuid.uuid4())


def now_utc() -> datetime.datetime:
    return datetime.datetime.now(pytz.UTC)


def now_date() -> datetime.date:
    return datetime.date.today()


def full_name(user: User) -> str:
    return f"{user.first_name or ''} {user.last_name or ''}"
