import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {Chat, Rights} from "../models/File";
import {fetchAllUsers, fetchRightsDescription, fetchRightsUserById, postSetRightsUser} from "./thunks/rightsThunks";

type UserType = { id: string, name: string };
const initialState = {
    users: null as null | UserType[],
    modal: {
        idUser: null as string | null,
        isOpen: false,
        name: null as undefined | null | string,
        rights: [] as Array<Rights>,
    },
    allRights: [] as Array<{ name: string, id: number }>,
}

export const managePanelSlice = createSlice({
    name: "manage-panel",
    initialState,
    reducers: {
        openModal(state, payload: PayloadAction<{ id: string }>) {
            state.modal.isOpen = true;
            state.modal.idUser = payload.payload.id;
            state.modal.name = state.users?.find((e) => e.id === payload.payload.id)?.name;
        },
        closeModal(state) {
            state.modal = {
                idUser: null,
                isOpen: false,
                name: null,
                rights: []
            }
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

        [fetchRightsDescription.fulfilled.type]: (state, action: PayloadAction<Array<{ name: string, id: number }>>) => {
            state.allRights = action.payload;
        },
        [fetchRightsDescription.pending.type]: (state, action: PayloadAction) => {
        },
        [fetchRightsDescription.rejected.type]: (state) => {
        },


        [fetchRightsUserById.fulfilled.type]: (state, action: PayloadAction<Array<number>>) => {
            state.modal.rights = action.payload;
        },
        [fetchRightsUserById.pending.type]: (state, action: PayloadAction) => {
        },
        [fetchRightsUserById.rejected.type]: (state) => {
        },


        [postSetRightsUser.fulfilled.type]: (state) => {
            state.modal = {idUser: null, isOpen: false, name: null, rights: []};
        },
        [postSetRightsUser.pending.type]: (state, action: PayloadAction) => {
        },
        [postSetRightsUser.rejected.type]: (state) => {
        },
    }
});


export default managePanelSlice.reducer;