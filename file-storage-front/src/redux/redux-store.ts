import {combineReducers} from "redux";
import {configureStore} from "@reduxjs/toolkit";
import filesReducer from "./filesSlice";

let rootReducer = combineReducers({
    filesReducer: filesReducer
});

export const setupStore = () => {
    return configureStore({
        reducer: rootReducer
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