import dataclasses


@dataclasses.dataclass
class TelegramFile:
    chat_id: int
    sender_id: str
    filename: str
    extension: str
    file_type: str
