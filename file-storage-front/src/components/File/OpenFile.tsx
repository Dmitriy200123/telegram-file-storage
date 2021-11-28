import React, {memo, useEffect} from 'react';
import "./File.scss"
import {ReactComponent as Svg} from "../../assets/download.svg";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {Category} from "../../models/File";
import {fetchFile} from "../../redux/actionsCreators";
import { Link } from 'react-router-dom';
import {useDispatch} from "react-redux";

export const OpenedFile:React.FC<any> = memo(({match}) => {
    const id = match.params["id"];
    const file = useAppSelector((state) => state.filesReducer.openFile);
    const dispatch = useDispatch();
    useEffect(() => {
        if (file && id === fileId)
            return;
        dispatch(fetchFile(id));
    }, [id])
    if (!file)
        return  null;
    const {fileName, fileId, fileType, sender, chat, uploadDate, downloadLink} = file;

    return (
        <div className={"file"}>
            <div className="file__header">
                <h2 className="file__title">{fileName}</h2>
                <Link className="file__close" to={"/files"}/>
            </div>
            <div className="file__content">
                <h3 className="file__content-title">Project1.jpg</h3>
                <div className="file__item"><span>Формат: </span>{Category[fileType]}</div>
                <div className="file__item"><span>Отправитель: </span><a>{sender.fullName}</a></div>
                <div className="file__item"><span>Чат: </span><a>{chat.name}</a></div>
                <div className="file__item"><span>Дата отправки: </span>{uploadDate}</div>
                <button className="file__btn"  onClick={() => {
                    window.open(downloadLink);
                }}>
                    <div>Скачать</div>
                    <Svg/></button>
            </div>
        </div>
    );
})




