import React, {FC} from 'react';
import './App.css';
import {BrowserRouter, Redirect, Route, Switch} from "react-router-dom";
import {Provider} from "react-redux";
import {setupStore} from "./redux/redux-store";
import {OpenedFile} from "./components/File/OpenFile";
import FilesMain from "./components/FilesMain/FilesMain";
import {useAppSelector} from "./utils/hooks/reduxHooks";
import {ModalConfirm} from "./components/utils/Modal/Modal";

const App: FC = () => {
    const {error,loading, modalConfirm} = useAppSelector((state) => state.filesReducer);
    const {isOpen, id} = modalConfirm;

    return (<div className="App">
        <header className="header"/>
        {!!error && error}
        {loading && "Загрузочка"}
        <Switch>
            <Route path={"/files"} exact component={FilesMain}/>
            <Route path={"/file"} component={OpenedFile}/>
            <Redirect to={"/files"}/>
        </Switch>
        {isOpen && id && <ModalConfirm id={id} />}
    </div>)
}

// const TestApp: FC = () => {
//     const a = {asd: "123"};
//     var file = new File([JSON.stringify(a)], 'foobar.txt');
//
//     let link = document.createElement('a');
//     link.text = "123";
//     link.download = file.name;
//
//     link.href = URL.createObjectURL(file);
//     link.click();
//     URL.revokeObjectURL(link.href);
//
//     return <div className="App">
//         <input type={"text"}/>
//
//     </div>
// }

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
