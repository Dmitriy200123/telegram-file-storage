from io import BytesIO
from telethon import TelegramClient
from telethon.events import NewMessage
from telethon.tl.custom import Message
from telethon.tl.types import MessageMediaDocument
from common.file_util import FileUtil
from common.interactor.loader_interactor import LoaderInteractor
from postgres.models.external_models import File
from postgres.models.external_models.file import type_map
from telegram_client_loader.handler.base_handler import BaseHandler


class TelegramLoader(BaseHandler):

    def __init__(
        self,
        telegram_client: TelegramClient,
        loader_interactor: LoaderInteractor
    ):
        super(TelegramLoader, self).__init__(telegram_client, loader_interactor)

        self.loader_interactor = loader_interactor
        self.run()

    def run(self):
        message: NewMessage = NewMessage()
        self.telegram_client.add_event_handler(self.__handle_new_message, message)

    def stop(self):
        self.telegram_client.remove_event_handler(self.__handle_new_message)

    async def __handle_new_message(self, event: NewMessage.Event):
        message: Message = event.message
        # is_valid = await self.is_valid_chat(message.chat_id)

        # if is_valid and message.media:
        if message.media:
            # MessageMediaWebPage
            telegram_file: File = self.__get_telegram_file(message)
            file: BytesIO = await self.download_file(message)
            await self.loader_interactor.save_file(telegram_file, file)

    @staticmethod
    def __get_telegram_file(message: Message) -> File:
        chat_id = message.chat_id
        sender_id = message.sender.id

        file_type, filename, extension = FileUtil.get_document_file_info(message.media) \
            if isinstance(message.media, MessageMediaDocument) \
            else FileUtil.get_photo_file_info(message)

        telegram_file = File(
            name=filename,
            extension=extension,
            type=type_map[file_type],
            upload_date=message.date,
            sender_telegram_id=sender_id,
            chat_telegram_id=chat_id,
        )

        return telegram_file
