import "./Navbar.scss";
import React, {FC, memo} from "react";
import {Link} from "react-router-dom";
import {ReactComponent as Logo} from "./../../assets/logo.svg";
import {ReactComponent as Download} from "./../../assets/download_2.svg";
import {ReactComponent as Search} from "./../../assets/search.svg";
import {ReactComponent as Settings} from "./../../assets/settings.svg";


export const Navbar: FC<{ className?: string }> = memo(({className}) => {
    return (<div className={(`${className} ` ?? "") + "navbar"}>
            <div className={"navbar__logo"}>
                <Logo/>
            </div>
            <div className={"navbar__links"}>
                <Link to={"/files"} className={"navbar__link"}><Search/>Поиск файлов</Link>
                <Link to={"/load/"} className={"navbar__link"}><Download/><span>Загрузка файлов</span></Link>
                <Link to={"/admin"} className={"navbar__link"}><Settings/><span>Предоставление доступа</span></Link>
            </div>
        </div>
    );
})




