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
import FilesMainCustomSelect from "./components/FilesMain/FilesMainCustomSelect";

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

    return <div className="App">
        <Switch>
            <Route path={"/files"} exact component={FilesMainCustomSelect}/>
            <Route path={"/file"} component={File}/>
            <Redirect to={"/files"}/>
        </Switch>
    </div>
}

const store = setupStore();

function FileStorageApp() {
    return (
        <BrowserRouter>
            <Provider store={store}>
                <TestApp/>
            </Provider>
        </BrowserRouter>
    );
}

export default FileStorageApp;
