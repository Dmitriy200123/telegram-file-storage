import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Link, Redirect, Route, Switch} from "react-router-dom";
import {Provider} from "react-redux";
import {setupStore} from "./redux/redux-store";
import {OpenedFile} from "./components/File/OpenFile";
import FilesMain from "./components/FilesMain/FilesMain";
import {useAppSelector} from "./utils/hooks/reduxHooks";
import {LoadFileMain} from "./components/LoadFile/LoadFileMain";
import {modalContents} from "./components/utils/Modal/Modal";
import {StartPage} from "./components/StartPage/StartPage";
import {Messages} from "./components/utils/Messages/Messages";

const App: FC = () => {
    const {filesReducer, profile} = useAppSelector((state) => state);
    const {messages} = profile;
    const {loading, modalConfirm} = filesReducer;
    const {isOpen, id, content} = modalConfirm;
    const Content = modalContents[content || 0];

    return (<div className="App" style={{display: "flex", flexDirection: "column", minHeight: "100vh"}}>
        {!!messages.length && <Messages messages={messages} className={"app__messages"}/>}
        <Switch>
            <Route path={"/login"} component={StartPage}/>
            <Route>
                <header className="header"/>
                <div style={{display: "grid", gap: 10}}>
                    <Link to={"/load/"}>Загрузить файл</Link>
                    <Link to={"/files"}>Искать файлы</Link>
                    <Link to={"/login"}>логин</Link>
                </div>
                {loading && <div>Загрузочка</div>}
                <div style={{flex: "1 1 auto", display: "flex", flexDirection: "column"}}>
                    <Switch>
                        <Route path={"/files"} exact component={FilesMain}/>
                        <Route path={"/file/:id"} component={OpenedFile}/>
                        <Route path={"/load/"} component={LoadFileMain}/>
                        <Route path={"/login"} component={StartPage}/>
                        <Redirect to={"/files"}/>
                    </Switch>
                </div>
                {isOpen && id && <Content id={id}/>}
            </Route>
        </Switch>
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
