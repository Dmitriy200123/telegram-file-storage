import "./Navbar.scss";
import React, {FC, memo} from "react";
import {Link} from "react-router-dom";
import {ReactComponent as Logo} from "./../../assets/logo.svg";
import {ReactComponent as Download} from "./../../assets/download_2.svg";
import {ReactComponent as Search} from "./../../assets/search.svg";
import {ReactComponent as Settings} from "./../../assets/settings.svg";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {Rights} from "../../models/File";


export const Navbar: FC<{ className?: string }> = memo(({className}) => {
    const {rights, hasTelegram} = useAppSelector((state) => state.profile);
    return (<div className={(`${className} ` ?? "") + "navbar"}>
            <div className={"navbar__logo"}>
                <Logo/>
            </div>
            <div className={"navbar__links"}>
                <Link to={"/profile"} className={"navbar__link"}><Settings/><span>Профиль</span></Link>
                {hasTelegram && <>
                    <Link to={"/files"} className={"navbar__link"}><Search/>Поиск файлов</Link>
                    {rights?.includes(Rights["Загружать файлы"]) &&
                    <Link to={"/load/"} className={"navbar__link"}><Download/><span>Загрузка файлов</span></Link>}
                    {rights?.includes(Rights["Редактировать права пользователей"]) &&
                    <Link to={"/admin"}
                          className={"navbar__link"}><Settings/><span>Предоставление доступа</span></Link>}
                </>
                }
                <Link to={"/docs/classes"}
                      className={"navbar__link"}><Settings/><span>Классификация документов</span></Link>
            </div>
        </div>
    );
})




