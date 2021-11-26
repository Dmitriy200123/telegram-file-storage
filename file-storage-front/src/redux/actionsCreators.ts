import {createAsyncThunk} from "@reduxjs/toolkit";
import {Category, Chat} from "../models/File";
import {fetchData, fetchConfig, fetchConfigText} from "./api";

export const fetchChats = createAsyncThunk("files/chats", async (_, thunkAPI) => {
    try {
        const data: Array<Chat> = await fetchData("/chats");
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
})

export const fetchFilters = createAsyncThunk("files/filters", async (_, thunkAPI) => {
    try {
        const chats = fetchData("/chats");
        const senders = fetchData("/senders");
        return {chats: await chats, senders: await senders};
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
})

type TypeFilesFetchFilters = {
    skip: number,
    take: number,
    categories?: Category
}
export const fetchFiles = createAsyncThunk("files/files", async (args: TypeFilesFetchFilters, thunkAPI) => {
    try {
        return await fetchConfig(`/files`, {params: args});
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
})

export const fetchRemoveFile = createAsyncThunk("files/remove", async (id: string, thunkAPI) => {
    try {
        return await fetchConfig(`/files/${id}`, {method: "DELETE"});
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить");
    }
});


export const fetchDownloadLink = async (id: string) => {
    try {
        const link = await fetchConfigText(`/files/${id}/downloadlink`, {method: "GET"});
        window.open(link);
    } catch (err) {

    }
}