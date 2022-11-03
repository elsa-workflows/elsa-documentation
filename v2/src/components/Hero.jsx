import { Fragment } from 'react'
import Image from 'next/future/image'
import clsx from 'clsx'
import Highlight, { defaultProps } from 'prism-react-renderer'

import { Button } from '@/components/Button'
import { HeroBackground } from '@/components/HeroBackground'
import blurCyanImage from '@/images/blur-cyan.png'
import blurIndigoImage from '@/images/blur-indigo.png'
import breakingBarriersImage from '@/images/undraw_breaking_barriers_vnf3.svg';

export function Hero() {
  return (
    <div className="overflow-hidden bg-slate-900 dark:-mb-32 dark:mt-[-4.5rem] dark:pb-32 dark:pt-[4.5rem] dark:lg:mt-[-4.75rem] dark:lg:pt-[4.75rem]">
      <div className="py-16 sm:px-2 lg:relative lg:py-20 lg:px-0">
        <div className="mx-auto grid max-w-2xl grid-cols-1 items-center gap-y-16 gap-x-8 px-4 lg:max-w-8xl lg:grid-cols-2 lg:px-8 xl:gap-x-16 xl:px-12">
          <div className="relative z-10 md:text-center lg:text-left">
            <Image
              className="absolute bottom-full right-full -mr-72 -mb-56 opacity-50"
              src={blurCyanImage}
              alt=""
              width={530}
              height={530}
              unoptimized
              priority
            />
            <div className="relative">
              <p className="inline bg-gradient-to-r from-green-600 via-emerald-400 to-emerald-800 bg-clip-text font-display text-5xl tracking-tight text-transparent">
                Open Source .NET Workflows Library.
              </p>
              <p className="mt-3 text-2xl tracking-tight text-slate-400">
                Enable workflow execution in any .NET application using code or a visual workflow designer.
              </p>
              <div className="mt-8 flex gap-4 md:justify-center lg:justify-start">
                <Button href="/docs/quickstarts/hello-world-console">Get started</Button>
                <Button href="https://github.com/elsa-workflows/elsa-core" variant="secondary">
                  View on GitHub
                </Button>
              </div>
            </div>
          </div>
          <div className="relative lg:static xl:pl-10">
            <div className="absolute inset-x-[-50vw] -top-32 -bottom-48 [mask-image:linear-gradient(transparent,white,white)] dark:[mask-image:linear-gradient(transparent,white,transparent)] lg:left-[calc(50%+14rem)] lg:right-0 lg:-top-32 lg:-bottom-32 lg:[mask-image:none] lg:dark:[mask-image:linear-gradient(white,white,transparent)]">
              <HeroBackground className="absolute top-1/2 left-1/2 -translate-y-1/2 -translate-x-1/2 lg:left-0 lg:translate-x-0 lg:translate-y-[-60%]" />
            </div>
            <div className="relative">
              <Image
                className="absolute -top-64 -right-64"
                src={blurCyanImage}
                alt=""
                width={530}
                height={530}
                unoptimized
                priority
              />
              <Image
                className="absolute -bottom-40 -right-44"
                src={blurIndigoImage}
                alt=""
                width={567}
                height={567}
                unoptimized
                priority
              />
              <div className="absolute inset-0 rounded-2xl bg-gradient-to-tr from-emerald-300 via-emerald-300/70 to-green-300 opacity-10 blur-lg" />
              <div className="absolute inset-0 rounded-2xl bg-gradient-to-tr from-emerald-300 via-emerald-300/70 to-green-300 opacity-10" />
              <div className="relative rounded-2xl bg-[#0A101F]/80 ring-1 ring-white/10 backdrop-blur">
                <div className="absolute -top-px left-20 right-11 h-px bg-gradient-to-r from-emerald-300/0 via-emerald-300/70 to-emerald-300/0" />
                <div className="absolute -bottom-px left-11 right-20 h-px bg-gradient-to-r from-green-400/0 via-green-400 to-green-400/0" />
                <div className="pl-4 pt-4">
                  <Image src={breakingBarriersImage} alt="" layout="fill" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
