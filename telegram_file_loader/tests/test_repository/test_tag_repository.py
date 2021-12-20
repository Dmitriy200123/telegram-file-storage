from tests.test_postgres_models.conftest import create_marked_text_tags
from tests.test_repository.conftest import create_message


async def test_get_marked_text(tag_repository):
    title = 'Custom title'
    description = 'Custom description'
    message = create_message(title, description)

    await create_marked_text_tags()

    title_actual, description_actual = await tag_repository.find_marked_text(message)

    assert title_actual == title
    assert description_actual == description


async def test_get_false_when_not_have_tags_in_db(tag_repository):
    message = create_message()

    has_tags = await tag_repository.has_tags(message)

    assert not has_tags


async def test_get_false_when_not_have_title_tag_in_message(tag_repository):
    message = 'Title *Description*'

    await create_marked_text_tags()

    has_tags = await tag_repository.has_tags(message)

    assert not has_tags


async def test_get_false_when_not_have_description_tag_in_message(tag_repository):
    message = '#Title# Description'

    await create_marked_text_tags()

    has_tags = await tag_repository.has_tags(message)

    assert not has_tags


async def test_get_true_when_have_tags_in_db(tag_repository):
    message = create_message()

    await create_marked_text_tags()

    has_tags = await tag_repository.has_tags(message)

    assert has_tags


async def test_get_empty_title_when_title_tag_incorrect(tag_repository):
    message = '#Incorrect title *Description*'

    await create_marked_text_tags()

    title, description = await tag_repository.find_marked_text(message)

    assert not title
    assert description


async def test_get_empty_description_when_description_tag_incorrect(tag_repository):
    message = '#Title# Incorrect Description*'

    await create_marked_text_tags()

    title, description = await tag_repository.find_marked_text(message)

    assert title
    assert not description


async def test_get_marked_text_when_description_tag_in_title(tag_repository):
    message = '#Title *Description*#'
    title_expected = 'Title *Description*'
    description_expected = 'Description'

    await create_marked_text_tags()

    title, description = await tag_repository.find_marked_text(message)

    assert title == title_expected
    assert description == description_expected


async def test_get_marked_text_when_multiply_tags(tag_repository):
    message = '#Title1# *Description1* #Title2# *Description2*'
    title_expected = 'Title1# *Description1* #Title2'
    description_expected = 'Description1* #Title2# *Description2'

    await create_marked_text_tags()

    title, description = await tag_repository.find_marked_text(message)

    assert title == title_expected
    assert description == description_expected
