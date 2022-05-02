import "./StartPage.scss";
import Logo from "./../../assets/logos/grey.png";
import {Button} from "../utils/Button/Button";
import {FC, memo, useEffect, useState} from "react";
import {GitlabAuth} from "gitlab-auth";
import {useAppDispatch} from "../../utils/hooks/reduxHooks";
import {fetchAuthGitlab} from "../../redux/thunks/profileThunks";

export const StartPage: FC = memo(() => {
    const dispatch = useAppDispatch();
    const [clicked, ChangeClicked] = useState(localStorage.getItem("flag") === "true");

    const changeClicked = (flag: boolean) => {
        localStorage.setItem("flag", String(flag));
        ChangeClicked(flag);
    };

    useEffect(() => {
        localStorage.setItem("flag", "false");
        let dataAuthGit = localStorage.getItem(`oidc.user:${process.env.REACT_APP_HOST}:${process.env.REACT_APP_ID}`);
        if (!dataAuthGit)
            dataAuthGit = sessionStorage.getItem(`oidc.user:${process.env.REACT_APP_HOST}:${process.env.REACT_APP_ID}`);

        if (dataAuthGit) {
            const json = JSON.parse(dataAuthGit);
            dispatch(fetchAuthGitlab(json.access_token));
        }
    }, [localStorage, sessionStorage]);

    return (
        <div>
            {clicked && <GitlabAuth
                host={process.env.REACT_APP_HOST as string}
                application_id={process.env.REACT_APP_ID as string}
                redirect_uri={process.env.REACT_APP_REDIRECT_URL as string}
                scope={"api openid profile email"}
                secret={process.env.REACT_APP_SECRET as string}
            />
            }
            <div className={"start-page"}>
                <div className={"start-page__logo"}>
                    <img src={Logo}/>
                </div>
                <div className={"start-page__content"}>
                    <h1 className={"start-page__title"}>Хранилище файлов</h1>
                    <p className={"start-page__description"}>— Сервис, позволяющий автоматически собирать файлы из чатов
                        Telegram и сохранять в хранилище с информацией о файле.</p>
                    <Button type={"white"} className={"start-page__btn"} onClick={() => changeClicked(true)}>Войти через
                        GitLab →</Button>
                </div>
            </div>
        </div>

    );
})




