import {InferActionsTypes} from "./redux-store";
import {Category, File} from "../types/types";


export type FormType<TValue> = { label: string, value: TValue };
let initialState = {
    files: [
        {
            fileName: "Файл",
            fileType: "video",
            chatId: 1,
            fileId: 2,
            uploadDate: "12.10.2020",
            downloadLink: "asdasdasd",
            senderId: 3
        },
        {
            fileName: "Файл2",
            fileType: "images",
            chatId: 2,
            fileId: 3,
            uploadDate: "13.10.2020",
            downloadLink: "asdasdasd",
            senderId: 4
        },
        {
            fileName: "Файл3",
            fileType: "links",
            chatId: 5,
            fileId: 5,
            uploadDate: "14.10.2020",
            downloadLink: "asdasdasd",
            senderId: 5
        },
    ] as Array<File>,
    form: {
        fileName: null as string | null | undefined,
        date: null as string | null | undefined,
        categories: null as Array<Category> | null | undefined,
        senders: null as Array<number> | null | undefined,
        chats: null as Array<number> | null | undefined,
    }
}

export type InitialStateType = typeof initialState;
type ActionsTypes = InferActionsTypes<typeof actions>;
// type ThunkType = BaseThunkType<ActionsTypes | ReturnType<typeof stopSubmit>>;

const filesReducer = (state = initialState, action: ActionsTypes): InitialStateType => {
    switch (action.type) {
        case 'file/CHANGE-FILTER-NAME':
            return {...state, form: {...state.form, fileName: action.fileName}}
        case 'file/CHANGE-FILTER-DATE':
            return {...state, form: {...state.form, date: action.date}}
        case 'file/CHANGE-FILTER-CATEGORIES':
            return {...state, form: {...state.form, categories: action.categories}}
        case 'file/CHANGE-FILTER-CHATS':
            return {...state, form: {...state.form, chats: action.chats}}
        case 'file/CHANGE-FILTER-SENDERS':
            return {...state, form: {...state.form, senders: action.senders}}
        default:
            return state;
    }
}

export const actions = {
    changeFilterFileName: (fileName: string | null | undefined) => {
        return {
            type: 'file/CHANGE-FILTER-NAME',
            fileName: fileName
        } as const
    },
    changeFilterDate: (date: string | null | undefined) => {
        return {
            type: 'file/CHANGE-FILTER-DATE',
            date: date
        } as const
    },
    changeFilterCategories: (categories: Array<Category> | null | undefined) => {
        return {
            type: 'file/CHANGE-FILTER-CATEGORIES',
            categories: categories
        } as const
    },
    changeFilterSenders: (senders: Array<number> | null | undefined) => {
        return {
            type: 'file/CHANGE-FILTER-SENDERS',
            senders: senders
        } as const
    },
    changeFilterChats: (chats: Array<number> | null | undefined) => {
        return {
            type: 'file/CHANGE-FILTER-CHATS',
            chats: chats
        } as const
    },
}


export default filesReducer;