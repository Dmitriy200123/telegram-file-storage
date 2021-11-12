import {createAsyncThunk} from "@reduxjs/toolkit";
import {Chat} from "../models/File";

const baseUrl = "https://localhost:5001/api";

export const fetchChats = createAsyncThunk("files/chats", async (_, thunkAPI) => {
    const response = await fetch(baseUrl + "/chats");
    if (!response.ok)
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    const data:Array<Chat> = await response.json();
    return data;
})