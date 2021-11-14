import datetime
import uuid

import pytz


def uuid_str() -> str:
    return str(uuid.uuid4())


def now_utc() -> datetime.datetime:
    return datetime.datetime.now(pytz.UTC)


def now_date() -> datetime.date:
    return datetime.date.today()
