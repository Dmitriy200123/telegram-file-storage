import React from 'react';
import "./File.scss"
import {ReactComponent as Svg} from "../../assets/download.svg";

const File = () => {
    return (
        <div className={"file"}>
            <div className="file__header">
                <h2 className="file__title">Файл</h2>
                <div className="file__close" />
            </div>
            <div className="file__content">
                <h3 className="file__content-title">Project1.jpg</h3>
                <div className="file__item"><span>Формат: </span>Изображение</div>
                <div className="file__item"><span>Отправитель: </span><a>Покровский Степан</a></div>
                <div className="file__item"><span>Чат: </span><a>office</a></div>
                <div className="file__item"><span>Дата отправки: </span>29.10.2020</div>
                <button className="file__btn"><div>Скачать</div><Svg/></button>
            </div>
        </div>
    );
}




export default File;
