import {AppDispatch} from "./redux-store";
import {createAsyncThunk} from "@reduxjs/toolkit";

export const fetchFiles = createAsyncThunk("files/filter",async (_, thunkAPI) => {
    try {
        const response = await fetch("url");
        return response.json();
    } catch (e) {
        return e;
    }
})