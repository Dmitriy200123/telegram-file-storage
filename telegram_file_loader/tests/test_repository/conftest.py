import pytest
from common.repository.tag_repository import TagRepository


def create_message(
    title: str = None,
    description: str = None,
    title_tag: str = None,
    description_tag: str = None
) -> str:
    title_tag = title_tag or '#'
    description_tag = description_tag or '*'
    return f"{title_tag}{title or 'Title'}{title_tag} some text " \
           f"{description_tag}{description or 'Description'}{description_tag}"


@pytest.fixture
def tag_repository(db_manager):
    return TagRepository(db_manager)
