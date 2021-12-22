from telethon.tl.types import (
    DocumentAttributeAudio,
    DocumentAttributeSticker,
    MessageMediaDocument,
    MessageMediaPhoto,
)


class Validator:

    @staticmethod
    def is_valid_media(media) -> bool:
        if isinstance(media, MessageMediaDocument):
            attributes = media.document.attributes
            invalid_types = filter(lambda attribute: Validator.__is_audio_voice(
                attribute) or Validator.__is_sticker(attribute), attributes)

            return not any(invalid_types)

        return isinstance(media, MessageMediaPhoto)

    @staticmethod
    def __is_audio_voice(attribute) -> bool:
        return isinstance(attribute, DocumentAttributeAudio) and attribute.voice

    @staticmethod
    def __is_sticker(attribute) -> bool:
        return isinstance(attribute, DocumentAttributeSticker)
