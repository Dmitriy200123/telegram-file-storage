import pytest
from tests.test_postgres_models.conftest import create_marked_text_tags


@pytest.fixture
async def init_tags():
    await create_marked_text_tags()


def create_message(title: str, description: str, title_tag: str = None, description_tag: str = None) -> str:
    title_tag = title_tag or '#'
    description_tag = description_tag or '*'
    return f"{title_tag}{title or 'Title'}{title_tag} some text " \
           f"{description_tag}{description or 'Description'}{description_tag}"
