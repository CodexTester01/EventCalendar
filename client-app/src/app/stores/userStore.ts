import { makeAutoObservable, runInAction } from "mobx";
import { User, UserFormValues } from "../models/user";
import agent from "../api/agent";
import { store } from "./store";
import { router } from "../router/Routes";

export default class UserStore {
    user: User | null = null;
    constructor() {
        makeAutoObservable(this)
    }

    get isLoggedIn() {
        return !!this.user;
    }

    login = async (creds: UserFormValues) => {
        try {
            const user = await agent.Account.login(creds)
            store.commonStore.setToken(user.token)
            runInAction(() => this.user = user)
            store.notificationStore.createHubConnection()
            router.navigate('/activities')
            store.modalStore.closeModal()
        } catch (error) {
            throw error
        }
    }
    register = async (creds: UserFormValues) => {
        try {
            const user = await agent.Account.register(creds)
            store.commonStore.setToken(user.token)
            runInAction(() => this.user = user)
            store.notificationStore.createHubConnection()
            router.navigate('/activities')
            store.modalStore.closeModal()
        } catch (error) {
            throw error
        }
    }

    logout = () => {
        store.commonStore.setToken(null)
        store.notificationStore.clear()
        this.user = null
        router.navigate('/') // home page redirection
    }
    getUser = async () => {
        try {
            const user = await agent.Account.current();
            runInAction(() => this.user = user)
            store.notificationStore.createHubConnection()
        } catch (error) {
            console.log(error)
        }
    }

    setImage = (image: string) => {
        if (this.user) this.user.image = image;
    }
    setDisplayName = (name: string) => {
        if (this.user) this.user.displayName = name;
    }
}