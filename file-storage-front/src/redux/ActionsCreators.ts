import {createAsyncThunk} from "@reduxjs/toolkit";

export const fetchFiles = createAsyncThunk("files/filter", async (_, thunkAPI) => {
    const response = await fetch("https://jsonplaceholder.typicode.com/pos2ts");
    if (!response.ok)
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    return response.json();

})