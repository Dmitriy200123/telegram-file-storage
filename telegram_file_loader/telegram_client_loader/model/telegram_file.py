import dataclasses
from datetime import datetime
from uuid import UUID

from postgres.models.db_models import FileTypeEnum
from postgres.models.external_models import File


@dataclasses.dataclass
class TelegramFile:
    chat_id: int
    sender_id: int
    filename: str
    extension: str
    file_type: str

    def to_file(self, chat_id: UUID, file_sender_id: UUID) -> File:
        return File(
            name=self.filename,
            extension=self.extension,
            type=self.get_type(),
            upload_date=datetime.now(),
            file_sender_id=file_sender_id,
            chat_id=chat_id,
        )

    def get_type(self):
        if self.file_type == 'document':
            return FileTypeEnum.Document

        if self.file_type == 'image':
            return FileTypeEnum.Image

        if self.file_type == 'video':
            return FileTypeEnum.Video

        if self.file_type == 'audio':
            return FileTypeEnum.Audio
