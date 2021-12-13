import "./StartPage.scss";
import Logo from "./../../assets/logos/grey.png";
import {Button} from "../utils/Button/Button";
import {FC, memo} from "react";
import {useHistory} from "react-router-dom";

export const StartPage: FC = memo(() => {
    const history = useHistory();
    return (
        <div className={"start-page"}>
            <div>
                <img src={Logo}/>
            </div>
            <div className={"start-page__content"}>
                <h1 className={"start-page__title"}>Хранилище файлов</h1>
                <p className={"start-page__description"}>— Сервис, позволяющий автоматически собирать файлы из чатов
                    Telegram и сохранять в хранилище с информацией о файле.</p>
                <Button type={"white"} className={"start-page__btn"} onClick={() => history.push("/")}>Войти через GitLab →</Button>
            </div>
        </div>
    );
})




