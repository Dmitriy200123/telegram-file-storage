from datetime import datetime
from telethon.tl.types import DocumentAttributeFilename, Message, MessageMediaDocument


class FileUtil:
    PHOTO_DATE_FORMAT = '%m-%d-%Y_%H-%M-%S'
    DEFAULT_PHOTO_EXTENSION = 'jpg'
    DEFAULT_PHOTO_MIME_TYPE = 'image'

    @staticmethod
    def get_document_file_info(media: MessageMediaDocument) -> (str, str, str):
        document = media.document
        file_type = FileUtil.__get_file_type(document.mime_type)
        filename_attribute = FileUtil.__find_filename_attribute(document.attributes)
        filename = filename_attribute.file_name
        extension = filename.split('.')[-1]

        return file_type, filename, extension

    @staticmethod
    def get_photo_file_info(message: Message) -> (str, str, str):
        file_type = FileUtil.DEFAULT_PHOTO_MIME_TYPE
        filename: str = FileUtil.get_photo_name(message.date)
        extension = FileUtil.DEFAULT_PHOTO_EXTENSION

        return file_type, filename, extension

    @staticmethod
    def get_photo_name(date=datetime.now()) -> str:
        return f'photo_{date.strftime(FileUtil.PHOTO_DATE_FORMAT)}.{FileUtil.DEFAULT_PHOTO_EXTENSION}'

    @staticmethod
    def __find_filename_attribute(attributes):
        return [attribute for attribute in attributes if isinstance(attribute, DocumentAttributeFilename)][-1]

    @staticmethod
    def __get_file_type(mime_type: str) -> str:
        telegram_file_type = mime_type.split('/')[0]

        if telegram_file_type == 'application' or telegram_file_type == 'text':
            return 'document'

        return telegram_file_type
