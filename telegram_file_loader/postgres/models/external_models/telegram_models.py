from pydantic import AnyUrl, BaseModel, Field, validator


class ValidateUrl(BaseModel):
    url: AnyUrl = Field(..., description='Урл для валидации')

    @validator('url', pre=True)
    def https_validator(cls, v: str):
        return v if '://' in v else f'http://{v}'
