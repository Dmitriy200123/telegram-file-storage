from tests.conftest import empty_classifications_response, full_classifications_response


async def test_get_classifications_success(classifications_documents_client, mock_response):
    mock_response.get(
        url='http://localhost:5003/api/documentClassifications?includeClassificationWords=True&skip=0&take=100',
        payload=full_classifications_response()
    )
    response = await classifications_documents_client.get_classifications()
    assert response == [
        {
            'id': '3fa85f64-5717-4562-b3fc-2c963f66afa6',
            'classificationWords': [
                {'value': 'string'}
            ]
        }
    ]


async def test_get_empty_classifications_success(classifications_documents_client, mock_response):
    mock_response.get(
        url='http://localhost:5003/api/documentClassifications?includeClassificationWords=True&skip=0&take=100',
        payload=empty_classifications_response()
    )
    response = await classifications_documents_client.get_classifications()
    assert response == []
