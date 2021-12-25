import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {Chat} from "../models/File";
import {fetchAllUsers, fetchRightsDescription} from "./thunks/rightsThunks";
type UserType = {id: string, name: string};
const initialState = {
    users: null as null | UserType[],
    modal: {
        idUser: null as string | null,
        isOpen: false,
        name: "123" as null | string,
    },
    allRights: null as null | Array<{name:string, id: number}>,
}

export const managePanelSlice = createSlice({
    name: "manage-panel",
    initialState,
    reducers: {
        openModal(state, payload: PayloadAction<{ id: string }>) {
            state.modal.isOpen = true;
            state.modal.idUser = payload.payload.id;
        },
        closeModal(state) {
            state.modal.isOpen = false;
            state.modal.idUser = null;
        },
    },
    extraReducers: {
        [fetchAllUsers.fulfilled.type]: (state, action: PayloadAction<Array<UserType>>) => {
            state.users = action.payload;
        },
        [fetchAllUsers.pending.type]: (state, action: PayloadAction) => {
        },
        [fetchAllUsers.rejected.type]: (state, action: PayloadAction<Array<Chat>>) => {
        },

        [fetchRightsDescription.fulfilled.type]: (state, action: PayloadAction<Array<{name:string, id: number}>>) => {
            state.allRights = action.payload;
        },
        [fetchRightsDescription.pending.type]: (state, action: PayloadAction) => {
        },
        [fetchRightsDescription.rejected.type]: (state) => {
        },
    }
});


export default managePanelSlice.reducer;