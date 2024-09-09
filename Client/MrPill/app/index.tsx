import { Redirect } from 'expo-router';
import DataHandler from '@/DataHandler';
import React, { useEffect } from 'react';

export default function IndexScreen() {

    useEffect(()=>{

        const loadUser = async () => {
            if (await DataHandler.loadUserFromStorage()) {
                DataHandler.setFlag("hasOpenedApp", true);
            }
        }
        loadUser();

    }, [])

    return <Redirect href={"/(login)/welcome"}/>;
}