from pydantic import AnyUrl, BaseModel, Field


class ValidateUrl(BaseModel):
    url: AnyUrl = Field(..., description='Урл для валидации')
