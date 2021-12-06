import {createSlice} from "@reduxjs/toolkit";


const initialState = {
    key: "",
    name: "Берсерк Справедливович",
    login: "",
    email: "",
    role: "",
    chats: "",
}

export const profileSlice = createSlice({
    name: "profile",
    initialState,
    reducers: {
        // openModal(state, payload:PayloadAction<{ id:string }>) {
        //     state.modalConfirm.isOpen = true;
        //     state.modalConfirm.id = payload.payload.id;
        // },
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


export default profileSlice.reducer;