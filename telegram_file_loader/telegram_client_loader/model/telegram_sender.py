from dataclasses import dataclass


@dataclass
class TelegramSender:
    id: int
    username: str
    fullname: str
