from pydantic import BaseModel


class BaseExternalModel(BaseModel):
    def dict_non_empty_fields(self):
        return self.dict(by_alias=True, exclude_none=True)
