from common.repository.base_repository import BaseRepository
from postgres.models.db_models.marked_text_tags import MarkedTextTags


class TagRepository(BaseRepository):

    async def has_tags(self, message: str) -> bool:
        tags: MarkedTextTags = (await self.adapter.get_all(model=MarkedTextTags))[-1]
        return tags.TitleTag in message and tags.DescriptionTag in message

    async def find_marked_text(self, message: str) -> (str, str):
        tags: MarkedTextTags = (await self.adapter.get_all(model=MarkedTextTags))[-1]
        title = self.__get_unwrapped_text(message, tags.TitleTag)
        description = self.__get_unwrapped_text(message, tags.DescriptionTag)

        return title, description

    @staticmethod
    def __get_unwrapped_text(message: str, tag: str) -> str:
        start_index = message.index(tag)
        start_index += len(tag)
        end_index = message.rindex(tag)
        unwrapped_text = message[start_index:end_index]

        return unwrapped_text
