import {createAsyncThunk} from "@reduxjs/toolkit";
import {Chat} from "../../models/File";
import {fetchConfig, fetchConfigText} from "../api/apiFiles";

export const fetchChats = createAsyncThunk("files/chats", async (_, thunkAPI) => {
    try {
        const data: Array<Chat> = await fetchConfig("/api/chats");
        return data;
    } catch (e) {
        return thunkAPI.rejectWithValue("Не удалось загрузить чаты");
    }
})

export const fetchFilters = createAsyncThunk("files/filters", async (_, thunkAPI) => {
    try {
        const chats = fetchConfig("/api/chats");
        const senders = fetchConfig("/api/senders");
        const filesNames = fetchConfig("/api/files/names");
        const countFiles = fetchConfigText("/api/files/count");
        return {chats: await chats, senders: await senders, countFiles: await countFiles, filesNames: await filesNames};
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить фильтры");
    }
})

type TypeFilesFetchFilters = {
    skip: number,
    take: number,
    categories?: string[] | null | undefined,
    dateTo?: string | null | undefined,
    dateFrom?: string | null | undefined,
    senderIds?: string[] | null | undefined,
    chatIds?: string[] | null | undefined,
}

export const fetchFiles = createAsyncThunk("files/files", async (args: TypeFilesFetchFilters, thunkAPI) => {
    try {
        const params:any = {...args};
        delete params['take'];
        delete params['skip'];
        const filesCount = fetchConfigText("/api/files/count", {params: params})
        return {files:await fetchConfig(`/api/files`, {params: args}), filesCount: await filesCount};
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить файлы");
    }
})


export const fetchFilesTypes = createAsyncThunk("files/filesTypes", async (_, thunkAPI) => {
    try {
        return await fetchConfig(`/api/files/types`);
    } catch (err) {
        return thunkAPI.rejectWithValue("Не удалось загрузить типы файлов");
    }
})

