import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Link, Redirect, Route, Switch} from "react-router-dom";
import {Provider} from "react-redux";
import {setupStore} from "./redux/redux-store";
import {OpenedFile} from "./components/File/OpenFile";
import FilesMain from "./components/FilesMain/FilesMain";
import {useAppSelector} from "./utils/hooks/reduxHooks";
import {ModalConfirm} from "./components/utils/Modal/Modal";
import {LoadFileMain} from "./components/LoadFile/LoadFileMain";

const App: FC = () => {
    const {error, loading, modalConfirm} = useAppSelector((state) => state.filesReducer);
    const {isOpen, id} = modalConfirm;

    return (<div className="App" style={{display:"flex", flexDirection: "column", minHeight:"100vh"}}>
        <header className="header"/>
        <div style={{display: "grid", gap: 10}}>
            <Link to={"/load/"}>Загрузить файл</Link>
            <Link to={"/files"}>Искать файлы</Link>
        </div>
        {!!error && <div style={{
            position: "fixed",
            right: 0,
            top: 100,
            background: "#ff061b",
            padding: 10,
            color: "white",
            borderRadius: 5
        }}>{error}</div>}
        {loading && <div>Загрузочка</div>}
        <div style={{flex:"1 1 auto", display:"flex", flexDirection: "column"}}>
            <Switch>
                <Route path={"/files"} exact component={FilesMain}/>
                <Route path={"/file/:id"} component={OpenedFile}/>
                <Route path={"/load/"} component={LoadFileMain}/>
                <Redirect to={"/files"}/>
            </Switch>
        </div>
        {isOpen && id && <ModalConfirm id={id}/>}
    </div>)
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
