import {createSlice, PayloadAction} from "@reduxjs/toolkit";


const initialState = {
    file: null as null | File
}

export const editorSlice = createSlice({
    name: "editor",
    initialState,
    reducers: {
        setFile(state, payload:PayloadAction<File>) {
            state.file = payload.payload;
        },
    },
    extraReducers: {
        // [fetchChats.fulfilled.type]: (state, action: PayloadAction<Array<Chat>>) => {
        //     state.chats = action.payload;
        //     state.loading =false;
        // },
        // [fetchChats.pending.type]: (state, action: PayloadAction) => {
        //     state.loading = true;
        // },
        // [fetchChats.rejected.type]: (state, action: PayloadAction<string>) => {
        //     state.error = action.payload
        // },

    }
});


export default editorSlice.reducer;