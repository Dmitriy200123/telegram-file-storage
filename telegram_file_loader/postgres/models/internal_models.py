from typing import List

from pydantic import BaseModel, Field


class ClassificationWords(BaseModel):
    value: str = Field(...)


class Classification(BaseModel):
    id: str
    classificationWords: List[ClassificationWords]
