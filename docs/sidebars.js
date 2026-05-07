// @ts-check

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
  tutorialSidebar: [
    'sistema-chat-privado',
    {
      type: 'category',
      label: 'Arquitectura',
      items: ['frontend', 'backend'],
    },
  ],
};

export default sidebars;
