import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { makeAutoObservable, runInAction } from "mobx";
import { Notification } from "../models/notification";
import { store } from "./store";

export default class NotificationStore {
    notifications: Notification[] = [];
    hubConnection: HubConnection | null = null;

    constructor() {
        makeAutoObservable(this);
    }

    createHubConnection = () => {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl(import.meta.env.VITE_NOTIFICATION_URL, {
                accessTokenFactory: () => store.userStore.user?.token as string
            })
            .withAutomaticReconnect()
            .configureLogging(LogLevel.Information)
            .build();

        this.hubConnection.start().catch(error => console.log('Error establishing connection : ', error));

        this.hubConnection.on('ReceiveNotification', (notification: Notification) => {
            runInAction(() => {
                this.notifications.unshift(notification);
            });
        });
    }

    stopHubConnection = () => {
        this.hubConnection?.stop().catch(error => console.log("Error stopping connection : ", error));
    }

    clear = () => {
        this.notifications = [];
        this.stopHubConnection();
    }
}
