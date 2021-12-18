from common.repository.tag_repository import TagRepository
from tests.test_repository.conftest import create_message


async def test_get_marked_text(db_manager):
    title = 'Custom title'
    description = 'Custom description'
    title_tag = '<t>'
    description_tag = '<d>'

    tag_repository = TagRepository(db_manager)
    message = create_message(title, description, title_tag, description_tag)

    title_actual, description_actual = await tag_repository.find_marked_text(message)

    assert title == title_actual
    assert description == description_actual
