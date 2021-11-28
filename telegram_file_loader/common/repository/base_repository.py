from postgres.pg_adapter import Adapter


class BaseRepository:
    adapter: Adapter

    def __init__(self, adapter: Adapter):
        self.adapter = adapter
