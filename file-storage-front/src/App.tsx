import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Redirect, Route, Switch} from "react-router-dom";
// import FilesMain from "./components/FilesMain/FilesMain";
import {Provider} from "react-redux";
import File from "./components/File/File";
import {setupStore} from "./redux/redux-store";
import FilesMain from "./components/FilesMain/FilesMain";
import Select from "./components/utils/Inputs/Select";
import {useForm} from "react-hook-form";

const App: FC = () => {
    return (<div className="App">
        <Switch>
            <Route path={"/files"} exact component={FilesMain}/>
            <Route path={"/file"} component={File}/>
            <Redirect to={"/files"}/>
        </Switch>
    </div>)
}

const TestApp = () => {
    const {register, handleSubmit, formState: {errors, dirtyFields: {}}, setValue, getValues} = useForm();

    const consoleLog = (e: any) => {
        console.log(e);
    }

    const onChangeForm = handleSubmit(consoleLog);

    return <form onChange={onChangeForm}>
        <Select register={register} name={"categories"} onChangeForm={onChangeForm} setValue={setValue}
                getValues={getValues}/>
        <Select register={register} name={"categories2"} onChangeForm={onChangeForm} setValue={setValue}
                getValues={getValues}/>
    </form>
}

const store = setupStore();

function FileStorageApp() {
    return (
        <BrowserRouter>
            <Provider store={store}>
                <App/>
            </Provider>
        </BrowserRouter>
    );
}

export default FileStorageApp;
