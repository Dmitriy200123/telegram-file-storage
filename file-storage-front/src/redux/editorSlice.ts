import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {postFile} from "./thunks/fileThunks";


const initialState = {
    file: null as null | File
}

export const editorSlice = createSlice({
    name: "editor",
    initialState,
    reducers: {
        setFile(state, payload:PayloadAction<File | null>) {
            state.file = payload.payload;
        },
    },
    extraReducers: {
        [postFile.fulfilled.type]: (state, _) => {
            state.file = null;
        },
        [postFile.pending.type]: (state, _) => {
        },
        [postFile.rejected.type]: (state, _) => {
            state.file = null;
        },

    }
});


export default editorSlice.reducer;