import {combineReducers} from "redux";
import {configureStore} from "@reduxjs/toolkit";
import filesReducer from "./filesSlice";
import profileReducer from "./profileSlice";
import editorReducer from "./editorSlice";
import {Api} from "../services/Api";

let rootReducer = combineReducers({
    filesReducer: filesReducer,
    profile: profileReducer,
    editor: editorReducer,
    [Api.reducerPath]: Api.reducer,
});

export const setupStore = () => {
    return configureStore({
        reducer: rootReducer,
        middleware: (getDefaultMiddleware) => getDefaultMiddleware({
            serializableCheck: {
                ignoredActions:["editor/setFile"]
            }
        }).concat(Api.middleware)
    })
}

export type RootState = ReturnType<typeof rootReducer>;
export type AppStore = ReturnType<typeof setupStore>;
export type AppDispatch = AppStore["dispatch"];

type RootReducerType = typeof rootReducer;
export type AppStateType = ReturnType<RootReducerType>;

// export type InferActionsTypes<T> = T extends { [key: string]: (...args: any[]) => infer U } ? U : never;
// export type BaseThunkType<ActionsTypes extends Action, R = Promise<void>> = ThunkAction<R, AppStateType, unknown, ActionsTypes>;
// const store = createStore(rootReducer, composeEnhancers(applyMiddleware(thunkMiddleware)));
// export default store;