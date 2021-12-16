from postgres.models.external_models.telegram_models import ValidateUrl


async def test_url():
    url1 = 'http://test1.com'
    url2 = 'http://test2.com:5000'
    url3 = 'http://127.0.0.1'
    url4 = 'http://www.test4.com:5000'
    url5 = 'tcp://www.test5.com:5000'
    url6 = 'www.test5.com:5000'

    url_model1 = ValidateUrl(url=url1)
    url_model2 = ValidateUrl(url=url2)
    url_model3 = ValidateUrl(url=url3)
    url_model4 = ValidateUrl(url=url4)
    url_model5 = ValidateUrl(url=url5)
    url_model5 = ValidateUrl(url=url6)

    print(123)
