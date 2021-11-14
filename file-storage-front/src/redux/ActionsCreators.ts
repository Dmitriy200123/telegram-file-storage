import {createAsyncThunk} from "@reduxjs/toolkit";
import {Chat} from "../models/File";

const baseUrl = "https://localhost:5001/api";

const fetchGetData = async (url: string) => {
    const response = await fetch(baseUrl + url);
    return await response.json();
};


export const fetchChats = createAsyncThunk("files/chats", async (_, thunkAPI) => {
    try {
        const response = await fetch(baseUrl + "/chats");
        const data: Array<Chat> = await response.json();
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
})


export const fetchFilters = createAsyncThunk("files/filters", async (_, thunkAPI) => {
    try {
        const chats = fetchGetData("/chats");
        const senders = fetchGetData("/senders");
        return {chats, senders};
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
})