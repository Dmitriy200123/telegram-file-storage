import React, {memo} from 'react';
import "./LoadFileMain.scss"
import {useDispatch} from "react-redux";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {LoadFile} from "./LoadFile/LoadFile";

export const LoadFileMain: React.FC<any> = memo(() => {
    // const file = useAppSelector((state) => state.editor.file);
    const dispatch = useDispatch();


    return (
        <div className={"load-file"}>
            <h2 className={"load-file__h2"}>Загрузка файлов</h2>
            <div className={"load-file__content"}>
                <LoadFile dispatch={dispatch} className={""}/>
            </div>
        </div>
    );
})




