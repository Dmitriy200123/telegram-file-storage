import React, {memo} from 'react';
import "./LoadFileMain.scss"
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {LoadFile} from "./LoadFile/LoadFile";
import {PreviewFile} from "./PreviewFile/PreviewFile";
import {Route, Switch} from "react-router-dom";
import {LoadText} from "./LoadText/LoadText";

export const LoadFileMain: React.FC<{ match: {path:string}}> = memo(({match}) => {
    const file = useAppSelector((state) => state.editor.file);
    return (
        <div className={"load-file"}>
            <h2 className={"load-file__h2"}>Загрузка файлов</h2>
            <Switch>
                <Route path={`${match.path}custom`} component={LoadText} />
                <Route path={`*`}>
                    <div className={"load-file__content"}>
                        {file
                            ? <PreviewFile file={file}/>
                            : <LoadFile className={""}/>
                        }
                    </div>
                </Route>
            </Switch>
        </div>
    );
})




