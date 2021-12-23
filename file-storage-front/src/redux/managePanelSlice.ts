import {createSlice, PayloadAction} from "@reduxjs/toolkit";
import {fetchChats} from "./thunks/mainThunks";
import {Chat, ModalContent} from "../models/File";
import {fetchAllUsers} from "./thunks/rightsThunks";
import {RightsManagerPanel} from "../components/RightsManagerPanel/RightsManagerPanel";
type UserType = {id: string, name: string};
const initialState = {
    users: null as null | UserType[],
    modal: {
        idUser: null as string | null,
        isOpen: false,
    }
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
        [fetchAllUsers.fulfilled.type]: (state, action: PayloadAction<Array<Chat>>) => {
            state.users = action.payload;
        },
        [fetchAllUsers.pending.type]: (state, action: PayloadAction) => {
        },
        [fetchAllUsers.rejected.type]: (state, action: PayloadAction<Array<Chat>>) => {
        },
    }
});


export default managePanelSlice.reducer;