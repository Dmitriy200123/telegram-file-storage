import {createAsyncThunk} from "@reduxjs/toolkit";
import {Chat} from "../models/File";

const baseUrl = "http://localhost:5001/api";

type ConfigType<T> = {
    params?:T
};
const fetchGetData = async (url: string, config?: any) => {
    const response = await fetch(baseUrl + url);
    return await response.json();
};

const fetchGetDataFiles = async (url: string, config?: any) => {
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
        const chats =  fetchGetData("/chats");
        const senders =  fetchGetData("/senders");
        // const  res = await Promise.all([chats, senders]);

        return {chats: await chats, senders: await senders };
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
})

export const fetchFiles = createAsyncThunk("files/files", async (_, thunkAPI) => {
    try {
        const files = await fetchGetDataFiles("/files?skip=0&take=5");

        return files;

    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
})